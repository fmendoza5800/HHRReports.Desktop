# Business Requirements Document
## HHR Reports System

---

**Document Version:** 1.0  
**Date:** December 2024  
**Project Name:** HHRReports  
**Document Status:** Final  

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Business Context](#2-business-context)
3. [Project Scope](#3-project-scope)
4. [Functional Requirements](#4-functional-requirements)
5. [Non-Functional Requirements](#5-non-functional-requirements)
6. [User Interface Requirements](#6-user-interface-requirements)
7. [Security Requirements](#7-security-requirements)
8. [Reporting Requirements](#8-reporting-requirements)
9. [Data Requirements](#9-data-requirements)
10. [System Integration](#10-system-integration)
11. [Performance Requirements](#11-performance-requirements)
12. [Constraints and Assumptions](#12-constraints-and-assumptions)
13. [Success Criteria](#13-success-criteria)
14. [Appendices](#14-appendices)

---

## 1. Executive Summary

### 1.1 Purpose
The HHRReports system is a web-based reporting application designed to provide comprehensive analytics and reporting capabilities for Historic Horse Racing (HHR) pool data. The system enables authorized users to generate, view, and export detailed reports on pool performance and terminal activities over specified time periods.

### 1.2 Business Objectives
- Provide real-time access to HHR pool performance data
- Enable data-driven decision making for gaming operations
- Ensure secure, role-based access to sensitive gaming data
- Streamline report generation and distribution processes
- Support regulatory compliance and audit requirements

### 1.3 Key Stakeholders
- **Wyoming Gaming Commission:** Primary regulatory body and system user
- **Gaming Operations Managers:** Daily operational reporting and analysis
- **Financial Analysts:** Revenue and performance tracking
- **IT Administrators:** System maintenance and user management
- **Compliance Officers:** Regulatory reporting and audit support

---

## 2. Business Context

### 2.1 Background
Historic Horse Racing (HHR) operations require sophisticated reporting capabilities to track pool performance, terminal activities, and gaming metrics. The current system aims to replace manual reporting processes and disparate data sources with a unified, web-based reporting platform.

### 2.2 Business Need
The gaming industry requires:
- **Real-time visibility** into pool performance metrics
- **Historical analysis** capabilities for trend identification
- **Regulatory compliance** through accurate and timely reporting
- **Operational efficiency** through automated report generation
- **Data security** to protect sensitive gaming information

### 2.3 Expected Benefits
- **Reduced reporting time** from hours to minutes
- **Improved data accuracy** through automated data collection
- **Enhanced decision-making** with real-time insights
- **Regulatory compliance** with standardized reporting formats
- **Cost savings** through process automation

---

## 3. Project Scope

### 3.1 In Scope
- Web-based reporting application accessible via modern browsers
- Three primary report types: Pool Details, Terminal Details, and Performance Report
- User authentication with database-level security
- 30-day rolling report generation (customizable end date for Performance Report)
- Excel export functionality
- Responsive design for various screen sizes
- Collapsible navigation for optimal screen utilization
- Execution timer display for report generation monitoring

### 3.2 Out of Scope
- Mobile native applications
- Report scheduling and automated distribution
- Data entry or modification capabilities
- Integration with third-party betting systems
- Historical data migration from legacy systems
- Multi-language support

### 3.3 System Boundaries
- **Data Source:** Azure SQL Database with HHR pool data
- **Access Method:** Web browser (Chrome, Edge, Firefox, Safari)
- **Geographic Scope:** Wyoming gaming operations
- **Time Period:** 30-day rolling window from selected date

---

## 4. Functional Requirements

### 4.1 Authentication and Authorization

#### FR-001: User Login
- System shall provide secure login functionality
- Users must authenticate with database credentials
- System shall validate user permissions against database stored procedures
- Failed login attempts shall be logged for security monitoring

#### FR-002: Session Management
- System shall maintain user sessions for authenticated users
- Sessions shall timeout after period of inactivity
- System shall provide secure logout functionality
- Session data shall be protected from unauthorized access

#### FR-003: Per-User Database Connections
- Each user session shall establish individual database connection
- Connection strings shall be dynamically generated based on user credentials
- Database permissions shall be validated at connection time

### 4.2 Report Generation

#### FR-004: Report Execution Timer
- System shall display execution timer during report generation
- Timer shall show elapsed time in HH:MM:SS format
- Timer shall persist after report completion showing final execution time
- Timer shall display status (Executing, Completed, Failed, Cancelled, Timeout)
- Timer shall appear in bottom-right corner of screen

#### FR-005: Pool Details Report
- System shall generate pool details for 30-day period
- Report shall display data from `usp_HHR_Pool_30` stored procedure
- Users shall specify end date for report period
- Report shall include all pool performance metrics

#### FR-006: Terminal Details Report
- System shall generate terminal details for 30-day period
- Report shall display data from `usp_HHR_TerminalDetail_30` stored procedure
- Users shall specify end date for report period
- Report shall include all terminal activity metrics

#### FR-007: Performance Report
- System shall generate performance metrics report
- Report shall display data from `usp_HHR_PerformanceReport` stored procedure
- Users shall specify end date for report period
- Report shall include revenue, transaction, and operational metrics
- Data shall display exactly as returned by stored procedure

#### FR-007: Report Filtering and Search
- System shall provide client-side search functionality
- Users shall filter report data by any column
- Search shall be case-insensitive
- Results shall update in real-time as user types

### 4.3 Data Display

#### FR-008: Pagination
- Large datasets shall be paginated for performance
- Users shall navigate between pages
- System shall display current page and total pages
- Users shall select number of records per page (10, 25, 50, 100)

#### FR-009: Data Virtualization
- System shall use virtualization for datasets exceeding 1000 rows
- Virtual scrolling shall maintain performance
- Visible data shall render on-demand

#### FR-010: Column Management
- Tables shall support dynamic column resizing
- Column widths shall adjust to content
- Users shall manually resize columns if needed

### 4.4 Export Functionality

#### FR-011: Excel Export
- System shall export report data to Excel format for all report types
- Export shall include all data (filtered/searched data for Pool Details)
- Excel files shall maintain proper data formatting (currency, dates, percentages)
- Export shall include report metadata (date, user, parameters)
- Export functionality available for Pool Details, Terminal Details, and Performance Reports
- Export shall show loading indicators during file generation

### 4.5 User Interface Navigation

#### FR-012: Sidebar Navigation
- System shall provide collapsible sidebar navigation
- Sidebar shall display navigation icons when collapsed
- Sidebar state shall persist across sessions
- Main content shall expand when sidebar is collapsed

#### FR-013: Theme Selection
- System shall support multiple color themes (White, Grey, Black)
- Theme selection shall persist per user session
- Theme shall apply consistently across all pages

---

## 5. Non-Functional Requirements

### 5.1 Performance

#### NFR-001: Page Load Time
- Initial page load shall complete within 3 seconds
- Subsequent navigation shall complete within 1 second
- Report generation shall begin within 5 seconds of request

#### NFR-002: Concurrent Users
- System shall support minimum 50 concurrent users
- Performance shall not degrade significantly with concurrent usage
- Database connection pooling shall optimize resource usage

### 5.2 Availability

#### NFR-003: System Uptime
- System shall maintain 99.5% uptime during business hours
- Planned maintenance shall occur outside business hours
- System shall provide graceful error handling

### 5.3 Scalability

#### NFR-004: Data Volume
- System shall handle reports with up to 100,000 rows
- Performance shall remain acceptable with large datasets
- System shall implement data virtualization for large results

### 5.4 Usability

#### NFR-005: Browser Compatibility
- System shall support Chrome (version 90+)
- System shall support Edge (version 90+)
- System shall support Firefox (version 88+)
- System shall support Safari (version 14+)

#### NFR-006: Responsive Design
- Interface shall adapt to screen sizes from 768px to 4K
- Tables shall remain readable on smaller screens
- Navigation shall remain accessible on all devices

---

## 6. User Interface Requirements

### 6.1 Layout Requirements

#### UI-001: Page Structure
- Consistent header with user information
- Collapsible sidebar navigation
- Main content area for reports
- Footer with system information

#### UI-002: Navigation Structure
- Home page as default landing
- Pool Details report page
- Terminal Details report page
- Performance Report page
- Login page for unauthenticated users

### 6.2 Visual Design

#### UI-003: Branding
- Application title: "HHRReports"
- Consistent color scheme based on selected theme
- Professional appearance suitable for business use

#### UI-004: Data Presentation
- Tabular format for report data
- Alternating row colors for readability
- Sortable column headers where applicable
- Clear data type formatting (dates, numbers, currency)

### 6.3 Interaction Design

#### UI-005: User Feedback
- Loading indicators during data retrieval
- Success messages for completed actions
- Error messages with actionable information
- Progress indicators for long-running operations

#### UI-006: Form Controls
- Date picker for date selection
- Dropdown menus for option selection
- Search box with real-time filtering
- Buttons with clear action labels

---

## 7. Security Requirements

### 7.1 Authentication

#### SEC-001: Credential Management
- Passwords shall not be stored in application
- Database credentials shall be validated by SQL Server
- Failed authentication shall not reveal user existence

### 7.2 Authorization

#### SEC-002: Access Control
- Users shall only access authorized stored procedures
- Database permissions shall control data access
- Application shall validate permissions on each request

### 7.3 Data Protection

#### SEC-003: Data Transmission
- All data transmission shall use HTTPS
- Sensitive data shall not be logged
- Connection strings shall be protected

#### SEC-004: Session Security
- Session tokens shall be cryptographically secure
- Sessions shall be invalidated on logout
- Concurrent sessions shall be limited per user

### 7.4 Audit and Logging

#### SEC-005: Activity Logging
- User login/logout shall be logged
- Report generation shall be logged
- Errors and exceptions shall be logged
- Logs shall include timestamp and user information

---

## 8. Reporting Requirements

### 8.1 Pool Details Report

#### RPT-001: Pool Metrics
Report shall include:
- Site identification information
- Pool performance metrics
- Handle amounts
- Number of tickets
- Commission calculations
- Tax calculations
- Net revenue figures
- Percentage distributions

#### RPT-002: Pool Aggregation
- Data shall be aggregated by site
- Subtotals shall be calculated per location
- Grand totals shall be displayed

### 8.2 Terminal Details Report

#### RPT-003: Terminal Metrics
Report shall include:
- Terminal identification
- VGM (Video Gaming Machine) information
- Transaction counts
- Amount wagered
- Amount won
- Net revenue per terminal
- Activity timestamps

#### RPT-004: Terminal Aggregation
- Data shall be grouped by location
- Performance metrics per terminal
- Summary statistics per site

### 8.3 Performance Report

#### RPT-005: Performance Metrics
Report shall include:
- Location and terminal identification
- Date-based performance data
- Gross and net revenue figures
- Transaction counts and averages
- Payout and hold percentages
- Operating hours and revenue per hour
- Tax and commission amounts
- Category and region classification
- Status indicators

#### RPT-006: Performance Data Format
- All numeric data displayed as returned from stored procedure
- Support for comma-separated values in numeric fields
- No data transformation or calculation in application layer

### 8.4 Report Features

#### RPT-007: Date Range Selection
- Reports shall cover 30-day period ending on selected date
- Date selection shall use calendar control
- Historical data shall be available based on retention policy

#### RPT-008: Report Formatting
- Numeric values shall display as provided by stored procedure
- Currency values shall maintain original formatting (dollar sign with commas)
- Percentages shall display correctly (84.87% format, not 8,487.24%)
- Dates shall use consistent format (MM/DD/YYYY)
- Support for various numeric formats including comma-separated values
- Excel exports shall preserve formatting with proper number formats

---

## 9. Data Requirements

### 9.1 Data Sources

#### DATA-001: Primary Database
- Azure SQL Database hosting HHR pool data
- Read-only access for reporting
- Stored procedures for data retrieval

### 9.2 Data Retention

#### DATA-002: Historical Data
- Minimum 13 months of historical data
- Data shall be available for year-over-year comparison
- Archival strategy for older data

### 9.3 Data Quality

#### DATA-003: Data Integrity
- Reports shall reflect real-time database state
- No data caching that could show stale information
- Data validation at stored procedure level

### 9.4 Data Volume

#### DATA-004: Expected Volumes
- Pool Details: ~50,000-60,000 rows per 30-day period
- Terminal Details: ~30,000-40,000 rows per 30-day period
- Performance Report: Variable based on date range and data availability
- Growth rate: 10-15% annually

---

## 10. System Integration

### 10.1 Database Integration

#### INT-001: SQL Server Integration
- Direct connection to Azure SQL Database
- Stored procedure execution
- Connection pooling for efficiency
- Retry logic for transient failures

### 10.2 Authentication Integration

#### INT-002: Database Authentication
- SQL Server authentication
- No separate user management system
- Database roles determine access levels

### 10.3 Export Integration

#### INT-003: Excel Integration
- Client-side Excel file generation
- No server-side Office dependencies
- Compatible with Excel 2016 and later

---

## 11. Performance Requirements

### 11.1 Response Times

#### PERF-001: User Interface Response
- Button clicks: < 100ms feedback
- Page navigation: < 1 second
- Search/filter: < 500ms for results update

### 11.2 Throughput

#### PERF-002: Report Generation
- Pool Details: < 30 seconds for 30-day period
- Terminal Details: < 30 seconds for 30-day period
- Performance Report: < 60 seconds for full execution
- Excel export: < 10 seconds for typical report
- Database timeout: 20 minutes for long-running queries

### 11.3 Resource Usage

#### PERF-003: Server Resources
- Memory usage: < 2GB per application instance
- CPU usage: < 50% average during peak hours
- Database connections: Maximum 100 concurrent
- Command timeout: 20 minutes for stored procedures

### 11.4 Optimization

#### PERF-004: Performance Features
- Data virtualization for large datasets
- Client-side filtering to reduce server load
- Efficient stored procedures with proper indexing
- Connection pooling for database efficiency

---

## 12. Constraints and Assumptions

### 12.1 Technical Constraints
- Must use existing database stored procedures
- Cannot modify database schema
- Must work within Azure subscription limits
- Browser-based solution only

### 12.2 Business Constraints
- Must comply with gaming regulations
- Budget limitations for infrastructure
- Existing IT support capabilities
- Current database licensing

### 12.3 Assumptions
- Database stored procedures are optimized
- Network connectivity is reliable
- Users have modern web browsers
- Database contains accurate data
- Users have basic computer skills

---

## 13. Success Criteria

### 13.1 Functional Success
- All specified reports generate accurately
- User authentication works reliably
- Export functionality produces valid Excel files
- Search and filter features work as expected

### 13.2 Performance Success
- Reports generate within specified timeframes
- System supports concurrent user load
- User interface remains responsive
- No significant performance degradation over time

### 13.3 User Acceptance
- Positive feedback from primary stakeholders
- Reduced time to generate reports
- Improved data accessibility
- Successful regulatory compliance audits

### 13.4 Operational Success
- System stability with minimal downtime
- Manageable maintenance requirements
- Successful disaster recovery testing
- Effective monitoring and alerting

---

## 14. Appendices

### Appendix A: Glossary

| Term | Definition |
|------|------------|
| HHR | Historic Horse Racing - A form of pari-mutuel wagering on previously run horse races |
| Pool | Aggregated wagers from multiple terminals/locations |
| Handle | Total amount wagered |
| VGM | Video Gaming Machine |
| Terminal | Individual gaming machine/station |
| Commission | Operator's share of gaming revenue |
| SP | Stored Procedure - Database program for data retrieval |

### Appendix B: Stored Procedures

#### usp_HHR_Pool_30
- **Purpose:** Retrieves pool performance data for 30-day period
- **Parameters:** @StartDate (datetime)
- **Returns:** Pool metrics aggregated by site
- **Typical Row Count:** 50,000-60,000

#### usp_HHR_TerminalDetail_30
- **Purpose:** Retrieves terminal activity data for 30-day period
- **Parameters:** @StartDate (datetime)
- **Returns:** Terminal-level performance metrics
- **Typical Row Count:** 30,000-40,000

#### usp_HHR_PerformanceReport
- **Purpose:** Retrieves comprehensive performance metrics
- **Parameters:** @EndDate (datetime)
- **Returns:** Performance data with revenue, transaction, and operational metrics
- **Typical Row Count:** Variable based on data availability
- **Data Format:** String values for all numeric fields to support various formats

### Appendix C: User Roles

| Role | Description | Permissions |
|------|-------------|-------------|
| Report User | Standard user with read-only access | Execute reporting stored procedures |
| Administrator | System administrator | Full database access, user management |
| Auditor | Compliance and audit role | Read-only access to all reports and logs |

### Appendix D: Technology Stack

| Component | Technology | Version |
|-----------|------------|---------|
| Framework | ASP.NET Core | 8.0 |
| UI Framework | Blazor Server | 8.0 |
| Database | SQL Server (Azure SQL) | Latest |
| ORM | Entity Framework Core | 9.0 |
| CSS Framework | Bootstrap | 5.x |
| JavaScript | Vanilla JS | ES6+ |

### Appendix E: Report Samples

#### Pool Details Report Columns
- Site_Abbr
- Site_Name  
- Total_Handle
- Total_Tickets
- Commission_Amount
- Tax_Amount
- Net_Revenue
- Revenue_Percentage

#### Terminal Details Report Columns
- Terminal_ID
- VGM_Number
- Location_Name
- Wager_Count
- Amount_Wagered
- Amount_Won
- Net_Terminal_Income
- Last_Activity_Date

---

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | December 2024 | Project Team | Initial version |
| 1.1 | September 2025 | Project Team | Added Performance Report and execution timer features |
| 1.2 | September 2025 | Project Team | Added Terminal Details Excel export, fixed percentage formatting, enhanced error handling |
| 1.3 | September 2025 | Project Team | Desktop and Cloud version synchronization - achieved full UI/UX parity between deployment models |

---

## Approval

| Role | Name | Signature | Date |
|------|------|-----------|------|
| Business Sponsor | _____________ | _____________ | _____ |
| IT Manager | _____________ | _____________ | _____ |
| Project Manager | _____________ | _____________ | _____ |
| Compliance Officer | _____________ | _____________ | _____ |

---

*End of Document*